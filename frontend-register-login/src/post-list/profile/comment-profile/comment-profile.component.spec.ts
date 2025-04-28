import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentProfileComponent } from './comment-profile.component';

describe('CommentProfileComponent', () => {
  let component: CommentProfileComponent;
  let fixture: ComponentFixture<CommentProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentProfileComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
