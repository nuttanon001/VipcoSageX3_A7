import {
  Component, OnInit,
  Output, EventEmitter,
  ElementRef,  Input
} from "@angular/core";
// by importing just the rxjs operators we need, We"re theoretically able
// to reduce our build size vs. importing all of them.
// rxjs 6
// import { Observable, fromEvent } from "rxjs";
// rxjs 5
import { fromEvent } from 'rxjs';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-attach-file',
  template: `
    <input id="attachFile1" [disabled]="readOnly" type="file" class="m-0 p-0 form-control-file form-control-sm" multiple />
  `
})
export class AttachFileComponent implements OnInit {
  @Input() readOnly: boolean = false;
  @Output() results: EventEmitter<FileList> = new EventEmitter<FileList>();
  /** attact-file ctor */
  constructor(private el: ElementRef) { }

  /** Called by Angular after attact-file component initialized */
  ngOnInit(): void {
    // debug here
    //Observable.fromEvent(this.el.nativeElement, "change")
    //  .debounceTime(250) // only once every 250ms
    //  .subscribe((file: any) => {
    //    // debug here
    //    // console.log("Files:", file);
    //    this.results.emit(file.target.files);
    //  });

    const obs = fromEvent(this.el.nativeElement, 'change')
      .pipe(
        debounceTime(250), //only search after 250 ms
        distinctUntilChanged(),
        map((file: any) => {
          return file.target.files;
      })).subscribe(result => {
        this.results.emit(result);
      });
  }
}
