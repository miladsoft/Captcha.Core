export class CaptchaApiResponse {
  constructor(
    public CaptchaImgUrl: string = "",
    public CaptchaId: string = "",
    public CaptchaTextValue: string = "",
    public CaptchaTokenValue: string = ""
  ) { }
}
