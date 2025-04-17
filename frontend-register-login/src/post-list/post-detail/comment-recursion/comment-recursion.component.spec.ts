import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentRecursionComponent } from './comment-recursion.component';

describe('CommentRecursionComponent', () => {
  let component: CommentRecursionComponent;
  let fixture: ComponentFixture<CommentRecursionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentRecursionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentRecursionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
