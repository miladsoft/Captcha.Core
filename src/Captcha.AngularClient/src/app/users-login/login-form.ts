import { CaptchaBase } from "../dnt-captcha/dnt-captcha-base";

export class LoginForm extends CaptchaBase {
  constructor(
    public username: string,
    public password: string) { super(); }
}

