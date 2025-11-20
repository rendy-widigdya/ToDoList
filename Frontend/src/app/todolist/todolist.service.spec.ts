import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TodoListService } from './todolist.service';
import { Todo } from './todo.model';

describe('TodoListService', () => {
  let service: TodoListService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TodoListService],
    });

    service = TestBed.inject(TodoListService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should GET all todos', (done) => {
    const mock: Todo[] = [
      { id: '1', title: 'a', isDone: false, createdAt: new Date().toISOString() },
    ];

    service.getAll().subscribe((res) => {
      expect(res).toEqual(mock);
      done();
    });

    const req = httpMock.expectOne((r) => r.method === 'GET' && r.url.indexOf('/todolist') > -1);
    expect(req.request.method).toBe('GET');
    req.flush(mock);
  });

  it('should POST create', (done) => {
    const mock: Todo = {
      id: '2',
      title: 'new',
      isDone: false,
      createdAt: new Date().toISOString(),
    };

    service.create('new').subscribe((res) => {
      expect(res).toEqual(mock);
      done();
    });

    const req = httpMock.expectOne((r) => r.method === 'POST' && r.url.indexOf('/todolist') > -1);
    expect(req.request.body).toEqual({ title: 'new' });
    req.flush(mock);
  });

  it('should PUT update', (done) => {
    const id = '3';
    const todo: Todo = { id, title: 't', isDone: true, createdAt: new Date().toISOString() };

    service.update(id, todo).subscribe((res) => {
      expect(res).toBeTruthy();
      done();
    });

    const req = httpMock.expectOne(
      (r) => r.method === 'PUT' && r.url.indexOf(`/todolist/${id}`) > -1
    );
    expect(req.request.body).toEqual(todo);
    req.flush({}, { status: 204, statusText: 'No Content' });
  });

  it('should DELETE', (done) => {
    const id = '4';

    service.delete(id).subscribe((res) => {
      expect(res).toBeTruthy();
      done();
    });

    const req = httpMock.expectOne(
      (r) => r.method === 'DELETE' && r.url.indexOf(`/todolist/${id}`) > -1
    );
    req.flush({}, { status: 204, statusText: 'No Content' });
  });
});
