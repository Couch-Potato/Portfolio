import { ControllerInstance, Message, MessageBus } from "./controller.js"
import busControllers from "./index.js"
export class PersistentBusController {
    /**
     * @type {Array<ControllerInstance>}
     */
    Controllers = []
    constructor(messageBus) {
        for (var x of busControllers) {
            this.Controllers.push(x.factory.GetController(x.channel, messageBus))
        }
        var bStream = messageBus.getBusStream();
        bStream.attach(async () => {
            var newData = await bStream.glance();
            var clout = []
            for (var i in newData) {
                var x = newData[i];
                for (var cont of this.Controllers) {
                    if (x.channel == cont.Channel) {
                        cont.Handle(new Message(x.channel, x.data, new MessageBus(messageBus)));
                        clout.push(i);
                    }
                }
            }
            // Clear any pipe messages.
            for (var x of clout) {
                bStream.clearOne(x);
            }
        })
    }
    static AttachToBus(bus) {
        return new this(bus)
    }
}