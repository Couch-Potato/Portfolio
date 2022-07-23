export class InviteEmail {
    Name;
    TokenUrl;
    Inviter;
    constructor(name, token, inviter) {
        this.Name = name;
        this.TokenUrl = token;
        this.Inviter = inviter;
    }
    Render() {
        return `<table style="background:#f3f3f3;width:100%;height: 100%;" cellpadding="0" cellspacing="0" border="0">
                          <tbody><tr>
                            <td style="padding: 50px;">
                              <table style="width: 550px;height: 100%;margin: 0 auto" cellpadding="0" cellspacing="0" border="0">
                                <tbody>
                                  <tr style="border-bottom:1px dashed #ddd">
                                    <td style="width: 175px;height: 20px;font-family: Roboto;font-size: 18px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 1.11;letter-spacing: normal;text-align: center;color: #001737;padding-bottom: 22px"> RoTools Invite </td>
                                  </tr>
                                  
                                  <tr>
                                    <td style="border-radius: 10px;background: #fff;padding: 30px 60px 20px 60px;margin-top: 10px;display: block;">
                                      <p style="font-family: Roboto;font-size: 18px;font-weight: 500;font-style: normal;font-stretch: normal;line-height: 1.11;letter-spacing: normal;color: #2b80ff;"> Invite </p>
                                      <p style="font-family: Roboto;font-size: 14px;font-weight: 500;font-style:
                                  normal;font-stretch: normal;line-height: 1.71;letter-spacing: normal;color: #001737;margin-bottom: 10px;"> Hi ${this.Name},</p>
                                      <p style="font-family: Roboto;font-size: 14px;font-weight: normal;font-style: normal;font-stretch: normal;line-height: 1.71;letter-spacing: normal;color: #001737;"> ${this.Inviter} has asked you join their RoTools workspace.</p>
                                      <a href="${this.TokenUrl}" style=" height: 34px;background-color: #2b80ff;border: none;color: #fff;padding: 8px 20px;border-radius: 4px;display: inline-block;margin-left: 10px;margin-bottom: 20px;">BEGIN</a>
                                      <p style="font-family: Roboto;font-size: 14px;font-weight: normal;font-style: normal;font-stretch: normal;line-height: 1.71;letter-spacing: normal;color: #bbbbbb;"> This is an automatically generated email please do not reply to this email.</p>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </td>
                          </tr>
                        </tbody></table>`
    }
}