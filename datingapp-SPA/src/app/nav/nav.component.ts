import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  jwtHelper = new JwtHelperService();
  decodedToken: any;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {}
  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('Logged in successfully');
        // const token = this.jwtHelper.decodeToken(localStorage.getItem('token'));
        // console.log(token);
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    // const token = this.jwtHelper.decodeToken(localStorage.getItem('token'));
    // console.log(token);
    return this.authService.loggedIn();
  }

  logout() {
    // const token = this.jwtHelper.decodeToken(localStorage.getItem('token'));
    // console.log(token);
    // console.log('before');
    localStorage.removeItem('token');
    this.alertify.message('Logged out');
    this.router.navigate(['/home']);
    // const token2 = this.jwtHelper.decodeToken(localStorage.getItem('token'));
    // console.log(token2);
    // console.log('after');
  }
}
