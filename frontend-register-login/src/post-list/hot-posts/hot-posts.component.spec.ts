import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HotPostsComponent } from './hot-posts.component';

describe('HotPostsComponent', () => {
  let component: HotPostsComponent;
  let fixture: ComponentFixture<HotPostsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HotPostsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HotPostsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
