import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError, defer, timer } from 'rxjs';
import { map } from 'rxjs/operators';
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
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
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
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
  });

  it('toggles done', fakeAsync(() => {
    const t: Todo = { id: '9', title: 'x', isDone: false, createdAt: new Date().toISOString() };
    svcSpy.getAll.and.returnValue(of([t]));
    svcSpy.update.and.returnValue(timer(10).pipe(map(() => undefined)));

    fixture.detectChanges();
    component.toggleDone(t);

    expect(svcSpy.update).toHaveBeenCalled();
    expect(component.loading).toBeTrue();
    tick(10);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
  }));

  it('deletes a todo', fakeAsync(() => {
    const t: Todo = { id: '10', title: 'rm', isDone: false, createdAt: new Date().toISOString() };
    svcSpy.getAll.and.returnValue(of([t]));
    svcSpy.delete.and.returnValue(timer(10).pipe(map(() => undefined)));

    fixture.detectChanges();
    component.delete(t);

    expect(svcSpy.delete).toHaveBeenCalledWith('10');
    expect(component.loading).toBeTrue();
    tick(10);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
  }));

  it('sets loading to true during service calls', () => {
    const data: Todo[] = [];
    let loadingStateDuringCall = false;
    
    svcSpy.getAll.and.callFake(() => {
      loadingStateDuringCall = component.loading;
      return defer(() => of(data));
    });

    component.load();
    expect(loadingStateDuringCall).toBeTrue();
  });

  it('handles load error', () => {
    const error = new Error('Network error');
    svcSpy.getAll.and.returnValue(throwError(() => error));

    component.load();

    expect(component.loading).toBeFalse();
    expect(component.error).toContain('Failed to load todos');
  });

  it('handles create error', () => {
    svcSpy.getAll.and.returnValue(of([]));
    const error = new Error('Create failed');
    svcSpy.create.and.returnValue(throwError(() => error));

    fixture.detectChanges();
    component.newTitle = 'test';
    component.add();

    expect(component.loading).toBeFalse();
    expect(component.error).toContain('Failed to create todo');
  });

  it('clears error on new operation', () => {
    svcSpy.getAll.and.returnValue(of([]));
    const error = new Error('First error');
    svcSpy.create.and.returnValue(throwError(() => error));

    fixture.detectChanges();
    component.newTitle = 'test';
    component.add();

    expect(component.error).toContain('Failed to create todo');

    // New operation should clear error
    svcSpy.create.and.returnValue(of({ id: '1', title: 'success', isDone: false, createdAt: new Date().toISOString() }));
    component.newTitle = 'success';
    component.add();

    expect(component.error).toBe('');
  });
});
