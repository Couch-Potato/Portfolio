require('dotenv').config();
const Discord = require('discord.js');
const bot = new Discord.Client();
const ws = require("ws");

const TOKEN = process.env.TOKEN;

bot.on('ready', () => {
    console.info(`Logged in as ${bot.user.tag}!`);
});

bot.on('message', msg => {
    if (msg.content.includes(" ")) {
        var cdata = msg.content.split(' ');
        var cmd = cdata[0];

        if (cmd == "!destroy") {
            var ccli = new ws(process.env.WS_CONNECTION);
            ccli.on("open", ()=>{
                ccli.on("message", (data)=>{
                    var parsed = JSON.parse(data);
                    if (parsed.Data.done) {
                        const embed = new Discord.MessageEmbed()
                            .setColor("#FFBF00")
                            .setTitle("Instance Destroyed.")
                            .addFields({
                                name: "Instance Id", value: cdata[1]
                            })
                            .setDescription("The instance has been destroyed.");
                        msg.channel.send(embed);
                    }else {
                        const embed = new Discord.MessageEmbed()
                            .setColor("#DE3163")
                            .setTitle("Operation Failure.")
                            .addFields({
                                name: "Instance Id", value: cdata[1]
                            })
                            .setDescription("The could not be destroyed.");
                        msg.channel.send(embed);
                    }
                })
                ccli.send(JSON.stringify({
                    Data: {
                        header:"terminate",
                        id:cdata[1]
                    }
                }))
            })
        }
        if (cmd == "!create") {
            var ccli = new ws(process.env.WS_CONNECTION);
            ccli.on("open", () => {
                ccli.on("message", (data) => {
                    var parsed = JSON.parse(data);
                    if (parsed.Data.status == "InstanceProvisioned") {
                        const embed = new Discord.MessageEmbed()
                            .setColor("#9FE2BF")
                            .setTitle("Instance Provisioned.")
                            .addFields({
                                name: "Hostname", value: cdata[1],
                            }, {
                                name:"Instance Id", value:parsed.Data.id
                            })
                            .setDescription("Your instance has been provisioned.");
                        msg.channel.send(embed);
                    } else if (parsed.Data.status == "RootCreated") {
                        const embed = new Discord.MessageEmbed()
                            .setColor("#6495ED")
                            .setTitle("Root Account Created.")
                            .addFields({
                                name: "Hostname", value: cdata[1],
                            }, {
                                name: "Instance Id", value: parsed.Data.id
                            }, {
                                name: "Root Account", value: `[Login as root](https://${cdata[1]}/onboarding?token=${parsed.Data.token})`
                            })
                            .setDescription("The root account for this instance has been generated. You can access it by clicking the link.");
                        msg.channel.send(embed);
                    }
                })
                ccli.send(JSON.stringify({
                    Data: {
                        header: "configure_server",
                        hostname: cdata[1]
                    }
                }))
            })
        }
        if (cmd == "!update") {
            const embed = new Discord.MessageEmbed()
                .setColor("#FFBF00")
                .setTitle("Rolling Update.")
                .setDescription("This may take a while.");
            msg.channel.send(embed);
            var ccli = new ws(process.env.WS_CONNECTION);
            ccli.on("open", () => {
                ccli.on("message", (data) => {
                    var parsed = JSON.parse(data);
                    const embed = new Discord.MessageEmbed()
                        .setColor("#9FE2BF")
                        .setTitle("Rolling Update.")
                        .setDescription("Rolling Update Completed.");
                    msg.channel.send(embed);
                })
                ccli.send(JSON.stringify({
                    Data: {
                        header: "update",
                    }
                }))
            })
        }
        if (cmd == "!replicas") {
            const embed = new Discord.MessageEmbed()
                .setColor("#FFBF00")
                .setTitle("Resizing instance.")
                .setDescription("This may take a while.");
            msg.channel.send(embed);
            var ccli = new ws(process.env.WS_CONNECTION);
            ccli.on("open", () => {
                ccli.on("message", (data) => {
                    var parsed = JSON.parse(data);
                    const embed = new Discord.MessageEmbed()
                        .setColor("#9FE2BF")
                        .setTitle("Instance resize.")
                        .setDescription("Rolling Update Completed.");
                    msg.channel.send(embed);
                })
                ccli.send(JSON.stringify({
                    Data: {
                        header: "set-replicas",
                        id:cdata[1],
                        replicas: cdata[2],
                    }
                }))
            })
        }
    }
});

bot.login(TOKEN);
