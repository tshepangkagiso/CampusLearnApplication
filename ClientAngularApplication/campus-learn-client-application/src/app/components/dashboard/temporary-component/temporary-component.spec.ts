import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemporaryComponent } from './temporary-component';

describe('TemporaryComponent', () => {
  let component: TemporaryComponent;
  let fixture: ComponentFixture<TemporaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemporaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemporaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
