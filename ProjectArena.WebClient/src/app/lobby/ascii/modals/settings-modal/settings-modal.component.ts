import { Component, OnInit, HostListener, ViewChild, OnDestroy } from '@angular/core';
import { AppFormGroup } from 'src/app/shared/components/form-group/app-form-group';
import { ComponentSizeEnum } from 'src/app/shared/models/enum/component-size.enum';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { UserManagementService } from 'src/app/auth/services/user-management.service';
import { UserService } from 'src/app/shared/services/user.service';
import { WebCommunicationService } from 'src/app/shared/services/web-communication.service';
import { controlRequiredValidator } from 'src/app/shared/validators/control-required.validator';
import { passwordDigitsValidator } from 'src/app/shared/validators/password-digits.validator';
import { passwordLowercaseValidator } from 'src/app/shared/validators/password-lowercase.validator';
import { passwordUppercaseValidator } from 'src/app/shared/validators/password-uppercase.validator';
import { controlMinLengthValidator } from 'src/app/shared/validators/control-min-length.validator';
import { confirmPasswordValidator } from 'src/app/shared/validators/confirm-password.validator';
import { ChangePasswordAuthorizedRequest } from 'src/app/auth/models/change-password-authorized-request.model';
import { ArenaHubService } from 'src/app/shared/services/arena-hub.service';
import { IModal } from 'src/app/shared/interfaces/modal.interface';
import { OverlayRef } from '@angular/cdk/overlay';
import { Subject } from 'rxjs';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { LoadingService } from 'src/app/shared/services/loading.service';

@Component({
  selector: 'app-settings-modal',
  templateUrl: './settings-modal.component.html',
  styleUrls: ['./settings-modal.component.scss']
})
export class SettingsModalComponent implements OnInit, OnDestroy, IModal<any> {

  passwordForm: AppFormGroup;

  onClose = new Subject<any>();
  onCancel = new Subject<any>();

  loading: boolean;
  errors: string[];

  componentSizeEnum = ComponentSizeEnum;

  get userUniqueId() {
    return this.userService.user?.uniqueId;
  }

  constructor(
    private overlay: OverlayRef,
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService,
    private webCommunicationService: WebCommunicationService,
    private arenaHub: ArenaHubService,
    private loadingService: LoadingService
    ) { }

  close() {
    this.onClose.next();
    this.overlay.detach();
    this.overlay.dispose();
  }

  ngOnDestroy() {
    this.onClose.unsubscribe();
    this.onCancel.unsubscribe();
  }

  closeOnClick(event) {
    if (event.target !== event.currentTarget || (this.loading && !this.errors)) {
      return;
    }
    this.close();
  }

  ngOnInit(): void {
    this.passwordForm = new AppFormGroup({
      currentPassword: this.formBuilder.control('', [
        controlRequiredValidator($localize`:@@controls.email:Current password`)
      ]),
      password: this.formBuilder.control('', [
        passwordDigitsValidator($localize`:@@controls.password:New password`),
        passwordLowercaseValidator($localize`:@@controls.password:New password`),
        passwordUppercaseValidator($localize`:@@controls.password:New password`),
        controlMinLengthValidator($localize`:@@controls.password:New password`, 8)
      ]),
      confirmPassword: this.formBuilder.control('', [
        confirmPasswordValidator($localize`:@@controls.confirm-password:Confirm password`)
      ])
    });
  }

  changePassword() {
    if (this.loading) {
      return;
    }
    const errors = this.passwordForm.appErrors;
    if (errors.length > 0) {
      this.passwordForm.controls.password.setValue('');
      this.errors = errors;
      this.loading = true;
    } else {
      this.loading = true;
      this.webCommunicationService.put<ChangePasswordAuthorizedRequest, void>('api/user/password', {
        currentPassword: this.passwordForm.controls.currentPassword.value,
        password: this.passwordForm.controls.password.value
      })
      .subscribe(result => {
        this.passwordForm.controls.currentPassword.setValue('');
        this.passwordForm.controls.password.setValue('');
        this.passwordForm.controls.confirmPassword.setValue('');
        if (result.success) {
          this.loadingService.startLoading({
            title: 'Loading...'
          }).subscribe(() => {
            this.router.navigate(['auth/signin']);
            this.userService.user = undefined;
            this.userService.unauthorized = true;
            this.userService.passwordWasChanged = true;
            this.arenaHub.disconnect();
            this.close();
          });
        } else {
          this.errors = result.errors;
        }
      });
    }
  }

  logOff() {
    this.loading = true;
    this.webCommunicationService.delete('api/auth/signout')
    .subscribe((result) => {
      if (result.success) {
        this.loadingService.startLoading({
          title: 'Loading...'
        }).subscribe(() => {
          this.router.navigate(['auth/signin']);
          this.userService.unauthorized = true;
          this.userService.user = undefined;
          this.arenaHub.disconnect();
          this.close();
        });
      } else {
        this.errors = result.errors;
      }
    });
  }
}
