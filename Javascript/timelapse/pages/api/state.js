import { HandleSerial } from "../../modules/serial"

if (!global.selected)
    global.selected = 0

if (!global.playing)
    global.playing = 0

export default function handler(req, res) {
    res.status(200).json({
        selected: global.selected,
        playing: global.playing,
    })
  }
  

HandleSerial();