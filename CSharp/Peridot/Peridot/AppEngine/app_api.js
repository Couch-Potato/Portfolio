var Peridot = {}
Peridot.run = function (file, data, callback)
{
    var x = new XMLHttpRequest();
    x.onreadystatechange = function ()
    {
        if (this.readyState == 4 && this.status == 200)
        {
            callback(x.responseText);
        }
    }
    var a = ""
    for (i = 0; i < data.length; i++) {
        a += `&${data.key}=${data.value}`;
    }
    x.open("GET", `run?script=${file}${a}`, true);
    x.send();
}
