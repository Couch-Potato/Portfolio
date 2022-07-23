import { RandomString } from "../util.js";
import { Renderable } from "./Renderable.js";

export class Terminal extends Renderable {
    PipeId;
    TerminalId;
    constructor(pipe) {
        super();
        this.PipeId = pipe;
        this.TerminalId = "t" + RandomString(13);

        this.InlineScripts = [`serverId = "${this.PipeId}"
        var term = new Terminal();
        term.open(document.getElementById('${this.TerminalId}'));
        function runFakeTerminal() {
            webSocket = new WebSocket("wss://"+window.location.host+"/server/pipes/" + "${this.PipeId}");
            if (term._initialized) {
            return;
        }
        webSocket.onmessage = function (event) {
            term.write(event.data);
            inputEnabled = true;
            prompt(term);
        }
        isClosed = false;
        webSocket.onclose = function () {
            isClosed = true;
            term.write("\\n\\rConnection closed.");
            inputEnabled = false;
        }
        term._initialized = true;
        webSocket.onopen = function () {
            inputEnabled = true;
        }
        inputEnabled = false;
        var written = "";
        term.onData(e => {
            switch (e) {
                case '\\r': // Enter
                    if (isClosed || !inputEnabled) return;
                    term.write("\\r\\n");
                    webSocket.send(written + "\\n\\r");
                    inputEnabled = false;
                    written = "";
                    break;
                case '\\u0003': // Ctrl+C
                    if (!isClosed) {
                        inputEnabled = true;
                        prompt(term);
                    }
                    break;
                case '\\u007F': // Backspace (DEL)
                    // Do not delete the prompt
                    if (term._core.buffer.x > 2 && !isClosed && inputEnabled) {
                        term.write('\\b \\b');
                        written = written.slice(0, -1);
                    }
                    break;
                default: // Print all other characters for demo
                    if (inputEnabled) {
                        written += e;
                        term.write(e);
                    }

            }
        });
    }
        function prompt(term) {
    term.write("\\r\\n\\x1B[1;1;36m${this.PipeId}\\x1B[0m$ ");
}
runFakeTerminal();`];
this.Scripts = ["/assets/js/xterm.js"];
this.CSS = ["/assets/css/xterm.css"];


    }

    Render() {
        return `<div id="${this.TerminalId}"></div>`;
    }
}