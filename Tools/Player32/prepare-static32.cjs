// Final pixel export of the approved ImageGen reference. No resampling in Unity.
const fs = require('fs');
const path = require('path');
const sharp = require('C:/Users/Kamil/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root = path.resolve(__dirname, '../..');
const review = path.join(root, 'ArtReview/Player32');
const palette = {
  '.': [0,0,0,0], O: '#241b20', a:'#482619', b:'#65341e', c:'#874621', d:'#a76532',
  e:'#a45e3d', f:'#d88c55', g:'#f5b378', h:'#ffdaa0',
  i:'#284e74', j:'#367aaa', k:'#61a6d3', l:'#93ccee',
  m:'#32292a', n:'#514032', p:'#705338',
  q:'#55291f', r:'#813e27', s:'#ae5732', t:'#ce7745',
  u:'#956319', v:'#d09d2b', w:'#f2c65c',
  x:'#294d94', y:'#4c7bc7', z:'#e5f3f4'
};
for(const [key,v] of Object.entries(palette)) if(typeof v==='string') palette[key]=[...v.slice(1).match(/../g).map(n=>parseInt(n,16)),255];
const entries=Object.entries(palette).filter(([k])=>k!=='.');
function nearest(r,g,b){ let best='O',dist=Infinity; for(const [key,c] of entries){const d=2*(r-c[0])**2+3*(g-c[1])**2+(b-c[2])**2;if(d<dist){dist=d;best=key;}}return best; }
function raw(rows){const b=Buffer.alloc(32*32*4);for(let y=0;y<32;y++)for(let x=0;x<32;x++)b.set(palette[rows[y][x]],(y*32+x)*4);return b;}
async function main(){
  const {data,info}=await sharp(path.join(review,'Static32-reference.png')).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  const w=info.width,h=info.height,remove=new Uint8Array(w*h),queue=new Int32Array(w*h); let head=0,tail=0;
  function bg(i){const p=i*4,r=data[p],g=data[p+1],b=data[p+2];return data[p+3]<128 || (Math.max(r,g,b)-Math.min(r,g,b)<42&&Math.min(r,g,b)>105);}
  function add(i){if(i<0||i>=w*h||remove[i]||!bg(i))return;remove[i]=1;queue[tail++]=i;}
  for(let x=0;x<w;x++){add(x);add((h-1)*w+x);}for(let y=0;y<h;y++){add(y*w);add(y*w+w-1);}
  while(head<tail){const i=queue[head++],x=i%w,y=Math.floor(i/w);if(x)add(i-1);if(x<w-1)add(i+1);if(y)add(i-w);if(y<h-1)add(i+w);}
  const directions=['Right','Left','Up','Down'],frames=[];
  for(let dir=0;dir<4;dir++){
    const x0=Math.round(dir*w/4),x1=Math.round((dir+1)*w/4);let minX=w,minY=h,maxX=0,maxY=0;
    for(let y=0;y<h;y++)for(let x=x0;x<x1;x++)if(!remove[y*w+x]){minX=Math.min(minX,x);maxX=Math.max(maxX,x);minY=Math.min(minY,y);maxY=Math.max(maxY,y);}
    const tw=Math.round((maxX-minX+1)/(maxY-minY+1)*30),tx=Math.floor((32-tw)/2);
    const rows=Array.from({length:32},()=>Array(32).fill('.'));
    for(let y=0;y<30;y++)for(let x=0;x<tw;x++){
      const sx=Math.min(maxX,Math.floor(minX+(x+.5)*(maxX-minX+1)/tw));
      const sy=Math.min(maxY,Math.floor(minY+(y+.5)*(maxY-minY+1)/30));
      if(!remove[sy*w+sx]){const p=(sy*w+sx)*4;rows[y+1][tx+x]=nearest(data[p],data[p+1],data[p+2]);}
    }
    frames.push({name:directions[dir],rows:rows.map(r=>r.join(''))});
    console.log(directions[dir],{minX,minY,maxX,maxY,tw});console.log(rows.map(r=>r.join('')).join('\n'));
  }
  fs.writeFileSync(path.join(review,'static32-pixels.json'),JSON.stringify({palette,frames},null,2));
  const atlas=Buffer.alloc(128*32*4);
  for(let d=0;d<4;d++){const f=raw(frames[d].rows);for(let y=0;y<32;y++)f.copy(atlas,(y*128+d*32)*4,y*32*4,(y+1)*32*4);}
  await sharp(atlas,{raw:{width:128,height:32,channels:4}}).png().toFile(path.join(review,'static32-normalized.png'));
  await sharp(atlas,{raw:{width:128,height:32,channels:4}}).resize(1024,256,{kernel:'nearest'}).flatten({background:'#343b45'}).png().toFile(path.join(review,'static32-normalized-preview.png'));
}
main().catch(e=>{console.error(e);process.exit(1);});
