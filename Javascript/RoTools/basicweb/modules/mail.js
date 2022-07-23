import Mailgun from "mailgun.js";
import formData from "form-data";

const mailgun = new Mailgun(formData);

const mg = mailgun.client({
    username: 'api',
    key: '3ad21c4c3f04a5810a35f37e4ec8772b-f8faf5ef-67bec8d9',
})

import { InviteEmail } from "../components/Email.js";

export async function MailTo(username, email, token, inviter) {
    var joinLink = `https://${global.HOSTSET}/onboarding/user?token=${token}`;
    var mailTemplate = new InviteEmail(
        username,
        joinLink,
        inviter
    );
    mg.messages.create("mg.rotools.net", {
        from:"RoTools Onboarding <noreply@rotools.net>",
        to:[email],
        subject: `${inviter} has invited you to ${global.HOSTSET.replace(".rotools.net", "")}`,
        html:mailTemplate.Render(),
        text:"Join URL" + joinLink
    }).catch(err=> console.log(err));
}