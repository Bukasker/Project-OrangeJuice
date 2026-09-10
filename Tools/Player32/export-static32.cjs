// Package the approved four-view reference as editable, registered pixel layers.
// Input: final-resolution pixel samples, created by prepare-static32.cjs.
const fs=require('fs'), path=require('path'), zlib=require('zlib'), crypto=require('crypto');
const sharp=require('C:/Users/Kamil/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root=path.resolve(__dirname,'../..');
const review=path.join(root,'ArtReview/Player32');
const out=path.join(root,'Assets/Main/Art/Player/Static32');
const source=JSON.parse(fs.readFileSync(path.join(review,'static32-pixels.json'),'utf8'));
const P=source.palette;
const names=['Body','Arms','Pants','Boots','Greaves','Tunic','Shoulders','Belt','Brackets','Hair','Eyes'];
const polish=['Cialo','Rece','Spodnie','Buty','Nagolenniki','Tunika','Naramienniki','Pas','Oslony przedramion','Wlosy z kucykiem','Oczy'];
const groups={Body:'Oefgh',Arms:'Oefgh',Hair:'Oabcd',Tunic:'Oijkl',Shoulders:'Oijkl',Pants:'Omnp',Boots:'Oqrst',Greaves:'Oqrs',Belt:'Ouvw',Brackets:'Oqrs',Eyes:'Oxyz'};
const blank=()=>Array.from({length:32},()=>Array(32).fill('.'));
const raw=rows=>{const b=Buffer.alloc(4096);for(let y=0;y<32;y++)for(let x=0;x<32;x++)b.set(P[rows[y][x]],(y*32+x)*4);return b;};
const nearest=(code,set)=>{let best=set[0],dist=Infinity;for(const k of set){const c=P[code],p=P[k],d=2*(c[0]-p[0])**2+3*(c[1]-p[1])**2+(c[2]-p[2])**2;if(d<dist){dist=d;best=k;}}return best;};
function assign(dir,x,y,c){
  const skin='efght'.includes(c),blue='ijklyxz'.includes(c);
  if(dir==='Right'){
    if(y<=8||(y<=12&&x<=14)||(y>=12&&y<=16&&x<=11))return 'Hair';
    if(y<=13){if(y>=9&&y<=10&&x>=18&&x<=20&&blue)return 'Eyes';return 'Body';}
    if(y>=28)return 'Boots';if(y===27)return 'Greaves';if(y>=24)return 'Pants';
    if(x<=14){if(y>=19&&y<=21)return 'Brackets';if(y<=15||blue)return 'Shoulders';return 'Arms';}
    return 'Tunic';
  }
  if(dir==='Up'){
    if(y<=12||(x>=15&&x<=17&&y>=13&&y<=18))return 'Hair';
    if(y>=28)return 'Boots';if(y===27)return 'Greaves';if(y>=23)return 'Pants';
    if((x<=12||x>=19)&&y<=16&&blue)return 'Shoulders';
    if(x<=11||x>=20){if(y>=19&&y<=20)return 'Brackets';return 'Arms';}
    return 'Tunic';
  }
  if(y<=8)return 'Hair';
  if(y<=13){
    if(y<=11&&(c==='a'||c==='b'||c==='q'||c==='r'||c==='s')&&x>=11&&x<=19)return 'Hair';
    if(y>=10&&y<=11&&x>=12&&x<=19&&blue)return 'Eyes';
    return 'Body';
  }
  if(y>=28)return 'Boots';if(y===27)return 'Greaves';if(y>=24)return 'Pants';
  if(y<=17&&(x<=12||x>=19)&&blue)return 'Shoulders';
  if(x<=10||x>=21){if(y>=19&&y<=20)return 'Brackets';return 'Arms';}
  if(y>=18&&y<=21&&(x===11||x===20))return 'Brackets';
  return 'Tunic';
}
function pixel(grid,x,y,c){if(x>=0&&x<32&&y>=0&&y<32)grid[y][x]=c;}
function rect(grid,x0,y0,x1,y1,c){for(let y=y0;y<=y1;y++)for(let x=x0;x<=x1;x++)pixel(grid,x,y,c);}
function line(grid,x0,x1,y,c){rect(grid,x0,y,x1,y,c);}
function baseHead(layers,dir){
  const b=layers.Body;
  // Scalp and neck are reconstructed beneath the independent hair layer.
  const ranges=dir==='Right'?[[4,14,18],[5,13,19],[6,12,20],[7,12,20],[8,12,20],[9,12,20],[10,13,20],[11,14,19],[12,15,18],[13,15,17],[14,15,17]]:
    [[3,14,17],[4,12,19],[5,11,20],[6,11,20],[7,11,20],[8,11,20],[9,11,20],[10,12,19],[11,13,18],[12,14,17],[13,14,17],[14,14,17]];
  for(const [y,x0,x1]of ranges)for(let x=x0;x<=x1;x++)if(b[y][x]==='.')b[y][x]=(x===x0||x===x1)?'e':(x<x0+2?'f':'g');
}
function create(frame){
  const layers=Object.fromEntries(names.map(n=>[n,blank()])),dir=frame.name;
  for(let y=0;y<32;y++)for(let x=0;x<32;x++){
    const code=frame.rows[y][x];if(code==='.')continue;
    const name=assign(dir,x,y,code);layers[name][y][x]=nearest(code,groups[name]);
    if(name!=='Hair'&&name!=='Eyes'){
      const under=(name==='Arms'||name==='Brackets'||name==='Shoulders')?'Arms':'Body';
      layers[under][y][x]=name==='Body'||name==='Arms'?nearest(code,'Oefgh'):(code==='O'?'O':nearest(code,'efg'));
    }
  }
  baseHead(layers,dir);
  // Restore fabric hidden by the belt/ponytail, without expanding the garment.
  if(dir==='Right'){
    for(let y=14;y<=23;y++)for(let x=15;x<=18;x++)if(layers.Tunic[y][x]==='.')layers.Tunic[y][x]=x===15?'j':'k';
    line(layers.Belt,15,18,21,'v');pixel(layers.Belt,18,21,'w');
    // Consistent two-pixel eye at the final resolution.
    layers.Eyes=blank();pixel(layers.Eyes,19,9,'z');pixel(layers.Eyes,19,10,'x');
    pixel(layers.Body,18,9,'g');pixel(layers.Body,18,10,'g');
    // Tie and tail are part of Hair, never of the body or tunic.
    pixel(layers.Hair,10,12,'v');pixel(layers.Hair,11,12,'u');
  }else if(dir==='Up'){
    for(let y=13;y<=22;y++)for(let x=13;x<=18;x++)if(layers.Tunic[y][x]==='.')layers.Tunic[y][x]=x===13?'j':'k';
    line(layers.Belt,12,19,20,'v');pixel(layers.Belt,12,20,'u');
    // A clear narrow ponytail with one common tie colour.
    line(layers.Hair,15,16,12,'v');pixel(layers.Hair,17,12,'u');
    for(let y=13;y<=17;y++){pixel(layers.Hair,15,y,'a');pixel(layers.Hair,16,y,y<16?'c':'b');pixel(layers.Hair,17,y,'a');}
    pixel(layers.Hair,16,18,'a');
  }else{
    for(let y=14;y<=23;y++)for(let x=13;x<=18;x++)if(layers.Tunic[y][x]==='.')layers.Tunic[y][x]=x===13?'j':'k';
    line(layers.Belt,11,20,21,'v');pixel(layers.Belt,16,21,'w');pixel(layers.Belt,18,22,'u');pixel(layers.Belt,18,23,'u');
    layers.Eyes=blank();for(const x of [13,18]){pixel(layers.Eyes,x,10,'z');pixel(layers.Eyes,x,11,'x');}
    for(const x of [12,14,17,19])if(layers.Body[11][x]!=='.')pixel(layers.Body,x,11,'g');
  }
  // Keep the palette of each material tight; avoid speckled photo-like shading.
  for(const name of ['Hair','Tunic','Shoulders','Pants','Boots','Greaves','Brackets']){
    const src=layers[name].map(r=>[...r]);
    for(let y=1;y<31;y++)for(let x=1;x<31;x++){
      const c=src[y][x];if(c==='.'||c==='O'||(name==='Hair'&&y>=12))continue;
      const neighbors=[src[y-1][x],src[y+1][x],src[y][x-1],src[y][x+1]].filter(k=>k!=='.'&&k!=='O');
      const counts={};for(const k of neighbors)counts[k]=(counts[k]||0)+1;
      const majority=Object.entries(counts).sort((a,b)=>b[1]-a[1])[0];
      if(majority&&majority[1]>=3)layers[name][y][x]=majority[0];
    }
  }
  return {name:dir,layers};
}
function composite(frame,namesToUse=names){const rows=blank();for(const name of namesToUse)for(let y=0;y<32;y++)for(let x=0;x<32;x++){const p=frame.layers[name][y][x];if(p!=='.')rows[y][x]=p;}return rows;}
function u16(n){const b=Buffer.alloc(2);b.writeUInt16LE(n);return b;}
function str(s){const b=Buffer.from(s);return Buffer.concat([u16(b.length),b]);}
function chunk(type,data){const b=Buffer.alloc(6);b.writeUInt32LE(data.length+6);b.writeUInt16LE(type,4);return Buffer.concat([b,data]);}
function ase(frames,width=32,height=32,atlas=false){
  const layerChunks=names.map(n=>{const b=Buffer.alloc(16);b.writeUInt16LE(3);b[12]=255;return chunk(0x2004,Buffer.concat([b,str(n)]));});
  const tagHeader=Buffer.alloc(10);tagHeader.writeUInt16LE(frames.length);
  const tags=frames.map((f,i)=>{const b=Buffer.alloc(17);b.writeUInt16LE(i);b.writeUInt16LE(i,2);b[13]=85;b[14]=170;b[15]=220;return Buffer.concat([b,str(f.name)]);});
  const all=[];
  for(let fi=0;fi<(atlas?1:frames.length);fi++){
    const chunks=fi===0?[...layerChunks]:[];
    if(fi===0&&!atlas)chunks.push(chunk(0x2018,Buffer.concat([tagHeader,...tags])));
    for(let li=0;li<names.length;li++){
      const b=Buffer.alloc(20);b.writeUInt16LE(li);b[6]=255;b.writeUInt16LE(2,7);b.writeUInt16LE(width,16);b.writeUInt16LE(height,18);
      const pixels=atlas?makeAtlas(frames,names[li]):raw(frames[fi].layers[names[li]]);
      chunks.push(chunk(0x2005,Buffer.concat([b,zlib.deflateSync(pixels)])));
    }
    const b=Buffer.alloc(16);b.writeUInt32LE(16+chunks.reduce((s,c)=>s+c.length,0));b.writeUInt16LE(0xf1fa,4);b.writeUInt16LE(chunks.length,6);b.writeUInt16LE(150,8);
    all.push(Buffer.concat([b,...chunks]));
  }
  const h=Buffer.alloc(128);h.writeUInt32LE(128+all.reduce((s,b)=>s+b.length,0));h.writeUInt16LE(0xa5e0,4);h.writeUInt16LE(atlas?1:frames.length,6);h.writeUInt16LE(width,8);h.writeUInt16LE(height,10);h.writeUInt16LE(32,12);h.writeUInt32LE(1,14);h.writeUInt16LE(150,18);h[34]=1;h[35]=1;h.writeUInt16LE(32,40);h.writeUInt16LE(32,42);
  return Buffer.concat([h,...all]);
}
function makeAtlas(frames,layer){const atlas=Buffer.alloc(128*32*4);frames.forEach((f,i)=>{const b=raw(layer?f.layers[layer]:composite(f));for(let y=0;y<32;y++)b.copy(atlas,(y*128+i*32)*4,y*128,(y+1)*128);});return atlas;}
const guid=p=>crypto.createHash('md5').update('orangejuice-static32-v1:'+p.replaceAll('\\','/')).digest('hex');
function meta(p,multi=false){
  const rel=path.relative(out,p),s=multi?2:1;
  let t=`fileFormatVersion: 2\nguid: ${guid(rel)}\nTextureImporter:\n  internalIDToNameTable: []\n  externalObjects: {}\n  serializedVersion: 12\n  mipmaps:\n    enableMipMap: 0\n    sRGBTexture: 1\n  isReadable: 0\n  textureFormat: 1\n  maxTextureSize: 2048\n  textureSettings:\n    serializedVersion: 2\n    filterMode: 0\n    aniso: 0\n    mipBias: 0\n    wrapU: 1\n    wrapV: 1\n    wrapW: 1\n  nPOTScale: 0\n  spriteMode: ${s}\n  spriteExtrude: 0\n  spriteMeshType: 0\n  alignment: 9\n  spritePivot: {x: 0.5, y: 0.03125}\n  spritePixelsToUnits: 16\n  spriteBorder: {x: 0, y: 0, z: 0, w: 0}\n  alphaUsage: 1\n  alphaIsTransparency: 1\n  spriteGenerateFallbackPhysicsShape: 0\n  textureType: 8\n  textureShape: 1\n  platformSettings:\n  - serializedVersion: 3\n    buildTarget: DefaultTexturePlatform\n    maxTextureSize: 2048\n    resizeAlgorithm: 0\n    textureFormat: -1\n    textureCompression: 0\n    compressionQuality: 100\n    crunchedCompression: 0\n    allowsAlphaSplitting: 0\n    overridden: 0\n  spriteSheet:\n    serializedVersion: 2\n    sprites:${multi?'':' []'}\n`;
  if(multi)for(let i=0;i<4;i++){const name=['Right','Left','Up','Down'][i];t+=`    - serializedVersion: 2\n      name: ${name}\n      rect:\n        serializedVersion: 2\n        x: ${i*32}\n        y: 0\n        width: 32\n        height: 32\n      alignment: 9\n      pivot: {x: 0.5, y: 0.03125}\n      border: {x: 0, y: 0, z: 0, w: 0}\n      outline: []\n      physicsShape: []\n      tessellationDetail: 0\n      bones: []\n      spriteID: ${guid(rel+name)}\n      internalID: ${21300000+i*2}\n      vertices: []\n      indices: \n      edges: []\n      weights: []\n`;}
  t+='    outline: []\n    physicsShape: []\n    bones: []\n    spriteID: '+guid(rel+'sheet')+'\n    internalID: 0\n    vertices: []\n    indices: \n    edges: []\n    weights: []\n    secondaryTextures: []\n    nameFileIdTable:'+(multi?'':' {}')+'\n';
  if(multi)for(let i=0;i<4;i++)t+=`      ${['Right','Left','Up','Down'][i]}: ${21300000+i*2}\n`;
  t+='  spritePackingTag: \n  userData: Static32 - 32px cells; point; lossless; feet pivot\n  assetBundleName: \n  assetBundleVariant: \n';fs.writeFileSync(p+'.meta',t);
}
async function png(p,data,w=32,h=32,withMeta=true){fs.mkdirSync(path.dirname(p),{recursive:true});await sharp(data,{raw:{width:w,height:h,channels:4}}).png().toFile(p);if(withMeta)meta(p,w===128);}
async function main(){
  fs.mkdirSync(out,{recursive:true});
  const right=create(source.frames[0]);
  const left={name:'Left',layers:Object.fromEntries(names.map(n=>[n,right.layers[n].map(r=>[...r].reverse())]))};
  const frames=[right,left,create(source.frames[2]),create(source.frames[3])];
  for(const f of frames){
    await png(path.join(out,'Frames',f.name+'.png'),raw(composite(f)));
    await png(path.join(out,'BodyBase',f.name+'.png'),raw(composite(f,['Body','Arms','Eyes'])));
    for(const name of names)await png(path.join(out,'Layers',name,f.name+'.png'),raw(f.layers[name]));
    fs.mkdirSync(path.join(out,'Aseprite'),{recursive:true});fs.writeFileSync(path.join(out,'Aseprite',f.name+'.aseprite'),ase([f]));
  }
  await png(path.join(out,'Player32.png'),makeAtlas(frames),128,32);
  for(const n of names)await png(path.join(out,'LayerSheets',n+'.png'),makeAtlas(frames,n),128,32);
  fs.writeFileSync(path.join(out,'Player32.aseprite'),ase(frames));
  fs.writeFileSync(path.join(out,'Player32-LayerSheet.aseprite'),ase(frames,128,32,true));
  fs.writeFileSync(path.join(review,'static32-layers.json'),JSON.stringify({palette:P,frames},null,2));
  await sharp(makeAtlas(frames),{raw:{width:128,height:32,channels:4}}).resize(1024,256,{kernel:'nearest'}).png().toFile(path.join(review,'Static32-final-preview.png'));
  const allLayers=Buffer.alloc(128*32*(names.length+1)*4);
  for(let i=0;i<=names.length;i++){const b=makeAtlas(frames,i?names[i-1]:null);b.copy(allLayers,i*128*32*4);}
  await png(path.join(review,'Static32-layers-grid.png'),allLayers,128,32*(names.length+1),false);
  const manifest={frameWidth:32,frameHeight:32,pixelsPerUnit:16,pivot:{x:.5,y:.03125},directions:frames.map(f=>f.name),layers:names.map((name,i)=>({name,description:polish[i],sortingOrder:i})),alpha:[0,255],palette:Object.fromEntries(Object.entries(P).map(([k,v])=>[k,v]))};
  fs.writeFileSync(path.join(out,'layout.json'),JSON.stringify(manifest,null,2));
  console.log(JSON.stringify({output:out,frames:4,layers:names.length,frameSize:'32x32'},null,2));
}
main().catch(e=>{console.error(e);process.exit(1);});
