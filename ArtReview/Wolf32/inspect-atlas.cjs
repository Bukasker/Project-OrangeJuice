const fs = require('fs');
const sharp = require('C:/Users/Kamil/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const source = 'C:/Users/Kamil/.codex/generated_images/01a081a6-f669-7c02-ad49-04032a7e29cf/exec-e86baad2-9120-4181-90c4-7f98cec28a33.png';
(async () => {
  const {data, info} = await sharp(source).ensureAlpha().raw().toBuffer({resolveWithObject: true});
  const rows = [];
  for (let y=0; y<info.height; y++) {
    let count=0;
    for(let x=0;x<info.width;x++) if(data[(y*info.width+x)*4+3]>127) count++;
    rows.push(count);
  }
  const bands=[];
  for(let y=0;y<rows.length;y++) if(rows[y]>8) {
    const start=y;
    while(y+1<rows.length && rows[y+1]>8) y++;
    bands.push([start,y]);
  }
  const columns=bands.map(([top,bottom])=>{
    const counts=[];
    for(let x=0;x<info.width;x++) {
      let count=0;
      for(let y=top;y<=bottom;y++) if(data[(y*info.width+x)*4+3]>127) count++;
      counts.push(count);
    }
    const cuts=[0];
    for(let c=1;c<8;c++) {
      const nominal=c*info.width/8;
      let best=Math.round(nominal), score=Infinity;
      for(let x=Math.round(nominal-25);x<=nominal+25;x++) {
        const s=counts[x]*100+Math.abs(x-nominal);
        if(s<score) {score=s;best=x;}
      }
      cuts.push(best);
    }
    cuts.push(info.width);
    return cuts;
  });
  const result={width:info.width,height:info.height,bands,columns};
  console.log(JSON.stringify(result));
  fs.writeFileSync(__dirname+'/source-analysis.json',JSON.stringify(result,null,2));
})();
