// Mechanical sprite extraction, nearest-neighbor export and Unity asset packaging.
// All artwork comes from the saved ImageGen source; this script does not draw poses.
const fs = require('fs');
const path = require('path');
const crypto = require('crypto');
const sharp = require('C:/Users/Kamil/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root = path.resolve(__dirname, '../..');
const destination = path.join(root, 'Assets/Main/Art/Mobs/Forest Creatures/AngryGreyWolf');
const source = path.join(__dirname, 'Wolf-Generated-Source.png');
const analysis = JSON.parse(fs.readFileSync(path.join(__dirname,'source-analysis.json')));
const animations = [
  {name:'idle', fps:4, loop:true}, {name:'run', fps:10, loop:true},
  {name:'sleep', fps:2, loop:true}, {name:'attack', fps:10, loop:false},
  {name:'death', fps:6, loop:false}
];
const directions = ['down','up','left','right'];
const guid = name => crypto.createHash('sha256').update('OrangeJuice/AngryGreyWolf/'+name).digest('hex').slice(0,32);
const write = (file, value) => fs.writeFileSync(file, value.replace(/\r\n/g,'\n'));
const folder = name => {
  fs.mkdirSync(name,{recursive:true});
  if(!fs.existsSync(name+'.meta')) write(name+'.meta',`fileFormatVersion: 2\nguid: ${guid(path.relative(root,name))}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);
};
(async()=>{
  folder(destination);
  folder(path.join(destination,'Animations'));
  const {data,info}=await sharp(source).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  const atlas=Buffer.alloc(256*320*4);
  const frames=[];
  for(let row=0;row<10;row++) for(let col=0;col<8;col++) {
    const a=animations[Math.floor(row/2)];
    const direction=directions[(row%2)*2+Math.floor(col/4)];
    const frame=col%4;
    const [top,bottom]=analysis.bands[row];
    const left=analysis.columns[row][col],right=analysis.columns[row][col+1];
    const center=(col+.5)*info.width/8;
    for(let y=0;y<32;y++) for(let x=0;x<32;x++) {
      const sx=Math.floor(center+(x+.5-16)*4.8);
      const sy=Math.floor(bottom+(y+.5-29)*4.8);
      if(sx<left||sx>=right||sy<top||sy>bottom) continue;
      const si=(sy*info.width+sx)*4;
      const di=((row*32+y)*256+col*32+x)*4;
      // Binary alpha and indexed PNG are standard low-resolution sprite export settings.
      if(data[si+3]>=128) {
        data.copy(atlas,di,si,si+3);
        atlas[di+3]=255;
      }
    }
    frames.push({name:`Wolf_${a.name}_${direction}_${String(frame).padStart(2,'0')}`,animation:a.name,direction,index:frame,
      rect:{x:col*32,y:row*32,w:32,h:32},durationMs:1000/a.fps,fileID:21300000+(row*8+col)*2});
  }
  const atlasName='Wolf-Animations-32.png';
  const atlasPath=path.join(destination,atlasName);
  await sharp(atlas,{raw:{width:256,height:320,channels:4}}).png({palette:true,colours:16,dither:0}).toFile(atlasPath);
  await sharp(atlasPath).resize(1024,1280,{kernel:'nearest'}).png().toFile(path.join(__dirname,'Wolf-Atlas-Preview-4x.png'));

  // Follow the TextureImporter schema already used by this Unity 2021 project.
  const textureGuid=guid(atlasName);
  let template=fs.readFileSync(path.join(root,'Assets/Main/Art/Mobs/Farm Creatures/Duck/Duck_01.png.meta'),'utf8');
  template=template.replace(/guid: [a-f0-9]+/,`guid: ${textureGuid}`)
    .replace(/filterMode: 1/g,'filterMode: 0').replace(/spriteMode: 1/,'spriteMode: 2')
    .replace(/spritePixelsToUnits: 100/,'spritePixelsToUnits: 32')
    .replace(/textureCompression: 1/g,'textureCompression: 0')
    .replace(/spriteGenerateFallbackPhysicsShape: 1/,'spriteGenerateFallbackPhysicsShape: 0');
  const spriteRecords=frames.map(f=>`    - serializedVersion: 2
      name: ${f.name}
      rect:
        serializedVersion: 2
        x: ${f.rect.x}
        y: ${320-f.rect.y-32}
        width: 32
        height: 32
      alignment: 0
      pivot: {x: 0.5, y: 0.5}
      border: {x: 0, y: 0, z: 0, w: 0}
      outline: []
      physicsShape: []
      tessellationDetail: 0
      bones: []
      spriteID: ${guid(f.name)}
      internalID: ${f.fileID}
      vertices: []
      indices: 
      edges: []
      weights: []`).join('\n');
  template=template.replace('    sprites: []','    sprites:\n'+spriteRecords)
    .replace('    nameFileIdTable: {}','    nameFileIdTable:\n'+frames.map(f=>`      ${f.name}: ${f.fileID}`).join('\n'));
  write(atlasPath+'.meta',template);

  for(const a of animations) for(const direction of directions) {
    const selected=frames.filter(f=>f.animation===a.name&&f.direction===direction);
    const sequence=[...selected,a.loop?selected[0]:selected[3]];
    const name=`Wolf_${a.name}_${direction}`;
    const clip=`%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: ${name}
  serializedVersion: 6
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - curve:
${sequence.map((f,i)=>`    - time: ${Number((i/a.fps).toFixed(8))}\n      value: {fileID: ${f.fileID}, guid: ${textureGuid}, type: 3}`).join('\n')}
    attribute: m_Sprite
    path: 
    classID: 212
    script: {fileID: 0}
  m_SampleRate: ${a.fps}
  m_WrapMode: 0
  m_Bounds:
    m_Center: {x: 0, y: 0, z: 0}
    m_Extent: {x: 0, y: 0, z: 0}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {fileID: 0}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
    pptrCurveMapping:
${sequence.map(f=>`    - {fileID: ${f.fileID}, guid: ${textureGuid}, type: 3}`).join('\n')}
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {fileID: 0}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: ${Number((4/a.fps).toFixed(8))}
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: ${a.loop?1:0}
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
`;
    const file=path.join(destination,'Animations',name+'.anim');
    write(file,clip);
    write(file+'.meta',`fileFormatVersion: 2\nguid: ${guid(name+'.anim')}\nNativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: 7400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);
  }
  const manifest={image:atlasName,size:{w:256,h:320},frameSize:{w:32,h:32},columns:8,rows:10,
    coordinateOrigin:'top-left',pixelsPerUnit:32,animations,directions,frames};
  write(path.join(destination,'Wolf-Animations-32.json'),JSON.stringify(manifest,null,2)+'\n');
  write(path.join(destination,'Wolf-Animations-32.json.meta'),`fileFormatVersion: 2\nguid: ${guid('manifest')}\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);
  fs.copyFileSync(atlasPath,path.join(__dirname,atlasName));
  console.log(JSON.stringify({atlas:atlasPath,frames:frames.length,clips:animations.length*directions.length}));
})();
