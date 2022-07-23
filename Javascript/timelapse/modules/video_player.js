const spawn = require("child_process").spawn

getPlayer();

export function getPlayer(){
    if (global.video_player)
        return global.video_player;
    else {
        global.video_p_py = spaw    n("py", ["video.py"]);
        global.video_player = {
            setMedia:(link)=>{
                global.video_p_py.stdin.write(JSON.stringify({
                    cmd:"load",
                    data:link
                }) + "\n")
            },
            scrubRight:()=>{
                global.video_p_py.stdin.write(JSON.stringify({
                    cmd:"scrub_right"
                }) + "\n")
            },
            scrubLeft:()=>{
                global.video_p_py.stdin.write(JSON.stringify({
                    cmd:"scrub_left"
                }) + "\n")
            },
            togglePause:()=>{
                global.video_p_py.stdin.write(JSON.stringify({
                    cmd:"pause"
                }) + "\n")
            }
        }
        return global.video_player;
    }
}
