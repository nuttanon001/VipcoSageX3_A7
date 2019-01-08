// angular core
import { OnInit, OnDestroy, ViewContainerRef } from "@angular/core";
import { FormGroup } from "@angular/forms";
// rxjs
import { Subscription } from "rxjs";
import { BaseRestService } from "./base-rest.service";
import { BaseCommunicateService } from "./base-communicate.service";
import { BaseModel } from "./base-model.model";

export abstract class BaseInfoComponent<
  Model extends BaseModel,
  Service extends BaseRestService<Model>,
  ServiceCommunicate extends BaseCommunicateService<Model>>
  implements OnInit, OnDestroy {
  /*
   * Constructor
   */
  constructor(
    protected service: Service,
    protected communicateService: ServiceCommunicate,
  ) { }

  /*
   * Parameter
   */
  InfoValue: Model;
  InfoValueForm: FormGroup;
  subscription: Subscription;
  step = 0;
  denySave: boolean = false;
  /*
   * Methods
   */
  setStep(index: number) {
    this.step = index;
  }
  nextStep() {
    this.step++;
  }
  prevStep() {
    this.step--;
  }
  // on hook component
  ngOnInit(): void {
    this.subscription = this.communicateService.toChildEdit$.subscribe(
      (InfoValue: Model) => {
        if (InfoValue) {
          this.denySave = InfoValue.ReadOnly;
        }
        this.onGetDataByKey(InfoValue);
      });
  }
  // on hook component
  ngOnDestroy(): void {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }
  // on get data by key
  abstract onGetDataByKey(value: Model): void;
  // build form
  abstract buildForm(): void;
  // on value of form change
  onValueChanged(data?: any): void {
    if (!this.InfoValueForm) { return; }
    const form = this.InfoValueForm;
    // on form valid or not
    this.onFormValid(form.valid);
  }
  // on valid data
  onFormValid(isValid: boolean): void {
    if (isValid && !this.denySave) {
      this.InfoValue = this.InfoValueForm.value;
      this.communicateService.toParent(this.InfoValue);
    }
  }
}
