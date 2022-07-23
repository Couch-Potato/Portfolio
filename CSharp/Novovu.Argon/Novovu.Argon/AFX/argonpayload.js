var Elements = {
    $$BODY$$
};
function CreateElement(name, properties, parent = document.body) {
    elm = Elements[name];
    source = elm.Source
    for (var key in properties) {
        source = source.replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]).replace("%%" + key + "%%", properties[key]);
    }
    var node = document.createElement("div")
    node.innerHTML = source;
    parent.appendChild(node);
    return node.childNodes[0];
}

