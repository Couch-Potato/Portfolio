import SerialPort from 'serialport'
import Readline from '@serialport/parser-readline'
import { getPlayer } from './video_player';
import videos from '../pages/api/videos.json'


const SERIAL_COMMANDS = {
    SET_SELECTED:'1',
    SET_RATE:'2',
    SET_PLAY_PAUSE:'3',
    SET_PLAYING:'4'
}


function SerialHandler() {
    
    const port = new SerialPort('/dev/tty.usbmodem14101', { baudRate: 9600 });
    const parser = port.pipe(new Readline({ delimeter: '\n' }));

    port.on('open', () => {
        console.log("Serial connected")
        global.SendToFirmware = (data) => {
            port.write(data + "\n")
        }
    })
 
    parser.on('data', data => {
        if (data.charAt('0') == SERIAL_COMMANDS.SET_SELECTED){
            var vx = data.replace(SERIAL_COMMANDS.SET_SELECTED, '');
            global.selected = Number(vx);
            // console.log(global.selected)
        }
        if (data.charAt('0') == SERIAL_COMMANDS.SET_PLAYING){
            global.playing = global.selected;
            getPlayer().setMedia(videos[global.playing].video)
        }
        if (data.charAt('0') == SERIAL_COMMANDS.SET_PLAY_PAUSE) {
            getPlayer().togglePause()
        }
        if (data.charAt('0') == SERIAL_COMMANDS.SET_RATE)
        {
            var vx = data.replace(SERIAL_COMMANDS.SET_SELECTED, '');
            if (vx == '0') {
                getPlayer().scrubLeft();
            }else if (vx == '1') {
                getPlayer().scrubRight();
            }
        }
    })
}


export function HandleSerial() {
    if (!global.serialHandled)
        SerialHandler()
    global.serialHandled = true;
}