import axios from "axios";
const gameKey = "123";
const uuid = "123-456-789-1011"
async function get(url){
    return await axios.get(url, {
        headers:{
            secret:gameKey
        }
    });
}
async function post(url, data){
    return await axios.post(url, data, {
        headers: {
            secret: gameKey
        }
    })
}

var message_queue = []
var channel_handlers = {}
var token = ""
var iid = ""

async function turnOnServer() {
    var data = await post("http://localhost:8080/game/create-server", {
        uuid:uuid
    })
    token = data.data.token
    iid = data.data.internal_id
    return {id:data.internal_id, token:data.token};
}

function write(channel,message) {
    message_queue.push({
        channel:channel,
        data:message
    });
}
function handleChannel(channel, callback) {
    channel_handlers[channel]=callback;
}
async function poll() {
    return await get(`http://localhost:8080/game/rx?uuid=${uuid}&token=${token}`)
}

function mainloop() {
    setInterval(async ()=>{
        var data = await poll();
        if (data.data.status != 200){
            return console.log(`[ERROR] ${data.data.message}`)
        }
        for (var server of data.data.data) {
            if (server.server == iid) {
                for (var message of server.messages)
                    channel_handlers[message.channel](message.data)
            }
        }
        console.log(message_queue);
        await post(`http://localhost:8080/game/tx?uuid=${uuid}&token=${token}`, {data:[{
            server:iid,
            messages: message_queue
        }]});
        message_queue = [];
    }, 1000)
}

async function start(){
    await turnOnServer();
    mainloop();
}

handleChannel("terminal_pipe", (data)=>{
    
    var nLine = data.message.replace("\n\r","");
    var handled = false
    if (nLine == "dbg.padd"){
        write("player_join", {
            username:"siteowner",
            userid: 20419725
        })
        write("terminal_pipe", {
            pipe: data.pipe,
            message: "Added player 'siteowner'."
        })
        handled = true;
    }
    if (!handled)
    {
        write("terminal_pipe", {
            pipe: data.pipe,
            message: `Unknown command: ${nLine}`
        })
    }
    
})

start();