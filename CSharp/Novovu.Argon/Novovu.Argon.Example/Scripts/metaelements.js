var hdgels = GetElement("Heading");
hdgels.Construct((element, array) => {
    if (array.blah == "1234") {
        element.text = "howdy!";
    }
});

var Specific = get("relPageTitle");
Specific.text = "abcdefgh";

Specific.Add(CreateElement("Heading", {text="thiswasdynamiccaly created!"}))
