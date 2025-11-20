import { TestBed, ComponentFixture } from '@angular/core/testing';
import { of } from 'rxjs';
import { TodoListComponent } from './todolist.component';
import { TodoListService } from './todolist.service';
import { Todo } from './todo.model';

describe('TodoListComponent', () => {
  let fixture: ComponentFixture<TodoListComponent>;
  let component: TodoListComponent;
  let svcSpy: jasmine.SpyObj<TodoListService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('TodoListService', ['getAll', 'create', 'update', 'delete']);

    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [{ provide: TodoListService, useValue: spy }],
    }).compileComponents();

    svcSpy = TestBed.inject(TodoListService) as jasmine.SpyObj<TodoListService>;
    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
  });

  it('loads todos on init', () => {
    const data: Todo[] = [
      { id: '1', title: 't1', isDone: false, createdAt: new Date().toISOString() },
    ];
    svcSpy.getAll.and.returnValue(of(data));

    fixture.detectChanges(); // triggers ngOnInit

    expect(component.todos).toEqual(data);
    expect(svcSpy.getAll).toHaveBeenCalled();
  });

  it('adds a todo', () => {
    const created: Todo = {
      id: '2',
      title: 'new',
      isDone: false,
      createdAt: new Date().toISOString(),
    };
    svcSpy.getAll.and.returnValue(of([]));
    svcSpy.create.and.returnValue(of(created));

    fixture.detectChanges();
    component.newTitle = 'new';
    component.add();

    expect(svcSpy.create).toHaveBeenCalledWith('new');
    expect(component.todos.indexOf(created)).toBeGreaterThanOrEqual(0);
  });

  it('toggles done', () => {
    const t: Todo = { id: '9', title: 'x', isDone: false, createdAt: new Date().toISOString() };
    svcSpy.getAll.and.returnValue(of([t]));
    svcSpy.update.and.returnValue(of());

    fixture.detectChanges();
    component.toggleDone(t);

    expect(svcSpy.update).toHaveBeenCalled();
  });

  it('deletes a todo', () => {
    const t: Todo = { id: '10', title: 'rm', isDone: false, createdAt: new Date().toISOString() };
    svcSpy.getAll.and.returnValue(of([t]));
    svcSpy.delete.and.returnValue(of());

    fixture.detectChanges();
    component.delete(t);

    expect(svcSpy.delete).toHaveBeenCalledWith('10');
  });
});
