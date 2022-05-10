import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CaptchaComponent } from './../dnt-captcha/dnt-captcha.component';
import { FormPosterService } from './form-poster.service';
import { LoginComponent } from './login/login.component';
import { UsersLoginRoutingModule } from './users-login-routing.module';

@NgModule({
  imports: [CommonModule, FormsModule, UsersLoginRoutingModule],
  declarations: [LoginComponent, CaptchaComponent],
  providers: [FormPosterService]
})
export class UsersLoginModule {}
