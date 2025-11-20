import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { TodoListComponent } from './todolist.component';
import { TodoListService } from './todolist.service';
import { Todo } from './todo.model';

describe('TodoListComponent', () => {
  let fixture: ComponentFixture<TodoListComponent>;
  let component: TodoListComponent;
  let serviceSpy: jasmine.SpyObj<TodoListService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('TodoListService', ['getAll', 'create', 'update', 'delete']);

    await TestBed.configureTestingModule({
      imports: [TodoListComponent],
      providers: [{ provide: TodoListService, useValue: spy }],
    }).compileComponents();

    serviceSpy = TestBed.inject(TodoListService) as jasmine.SpyObj<TodoListService>;
    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('loads todos on init', fakeAsync(() => {
    const data: Todo[] = [
      { id: '1', title: 't1', isDone: false, createdAt: new Date().toISOString() },
    ];
    serviceSpy.getAll.and.returnValue(of(data));

    fixture.detectChanges(); // triggers ngOnInit
    tick(); // flush async operations

    expect(component.todos()).toEqual(data);
    expect(component.isLoading()).toBe(false);
    expect(serviceSpy.getAll).toHaveBeenCalled();
  }));

  it('adds a todo', fakeAsync(() => {
    const created: Todo = {
      id: '2',
      title: 'new',
      isDone: false,
      createdAt: new Date().toISOString(),
    };
    serviceSpy.getAll.and.returnValue(of([]));
    serviceSpy.create.and.returnValue(of(created));

    fixture.detectChanges();
    tick();

    component.newTitle = 'new';
    component.add();
    tick();

    expect(serviceSpy.create).toHaveBeenCalledWith('new');
    expect(component.todos()).toContain(created);
    expect(component.newTitle).toBe('');
    expect(component.isLoading()).toBe(false);
  }));

  it('does not add empty todo', () => {
    serviceSpy.getAll.and.returnValue(of([]));

    fixture.detectChanges();
    component.newTitle = '   ';
    component.add();

    expect(serviceSpy.create).not.toHaveBeenCalled();
  });

  it('toggles done', fakeAsync(() => {
    const t: Todo = { id: '9', title: 'x', isDone: false, createdAt: new Date().toISOString() };
    const updated: Todo = { ...t, isDone: true };
    serviceSpy.getAll.and.returnValue(of([t]));
    serviceSpy.update.and.returnValue(of(updated));

    fixture.detectChanges();
    tick();

    component.toggleDone(t);
    tick();

    expect(serviceSpy.update).toHaveBeenCalledWith('9', updated);
    expect(component.todos().find((todo) => todo.id === '9')?.isDone).toBe(true);
  }));

  it('deletes a todo', fakeAsync(() => {
    const t: Todo = { id: '10', title: 'rm', isDone: false, createdAt: new Date().toISOString() };
    serviceSpy.getAll.and.returnValue(of([t]));
    serviceSpy.delete.and.returnValue(of(void 0));

    fixture.detectChanges();
    tick();

    component.delete(t);
    tick();

    expect(serviceSpy.delete).toHaveBeenCalledWith('10');
    expect(component.todos().find((todo) => todo.id === '10')).toBeUndefined();
    expect(component.isLoading()).toBe(false);
  }));

  it('handles error when loading todos', fakeAsync(() => {
    const error = new Error('Network error');
    serviceSpy.getAll.and.returnValue(throwError(() => error));

    fixture.detectChanges();
    tick();

    expect(component.error()).toContain('Failed to load todos');
    expect(component.todos()).toEqual([]);
    expect(component.isLoading()).toBe(false);
  }));

  it('handles error when creating todo', fakeAsync(() => {
    serviceSpy.getAll.and.returnValue(of([]));
    const error = new Error('Network error');
    serviceSpy.create.and.returnValue(throwError(() => error));

    fixture.detectChanges();
    tick();

    component.newTitle = 'test';
    component.add();
    tick();

    expect(component.error()).toContain('Failed to create todo');
    expect(component.todos().length).toBe(0);
    expect(component.isLoading()).toBe(false);
  }));

  it('starts editing a todo', () => {
    const t: Todo = { id: '1', title: 'test', isDone: false, createdAt: new Date().toISOString() };
    serviceSpy.getAll.and.returnValue(of([t]));

    fixture.detectChanges();
    component.startEdit(t);

    expect(component.editingId).toBe('1');
    expect(component.editTitle).toBe('test');
  });

  it('cancels editing', () => {
    component.editingId = '1';
    component.editTitle = 'test';
    component.cancelEdit();

    expect(component.editingId).toBeNull();
    expect(component.editTitle).toBe('');
  });

  it('saves edited todo', fakeAsync(() => {
    const t: Todo = { id: '1', title: 'old', isDone: false, createdAt: new Date().toISOString() };
    const updated: Todo = { ...t, title: 'new' };
    serviceSpy.getAll.and.returnValue(of([t]));
    serviceSpy.update.and.returnValue(of(updated));

    fixture.detectChanges();
    tick();

    component.startEdit(t);
    component.editTitle = 'new';
    component.saveEdit(t);
    tick();

    expect(serviceSpy.update).toHaveBeenCalledWith('1', updated);
    expect(component.todos().find((todo) => todo.id === '1')?.title).toBe('new');
    expect(component.editingId).toBeNull();
  }));

  it('tracks todos by id', () => {
    const t: Todo = { id: '1', title: 'test', isDone: false, createdAt: new Date().toISOString() };
    expect(component.trackByTodoId(0, t)).toBe('1');
  });
});
