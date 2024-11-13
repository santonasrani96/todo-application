import { get, post, put, del } from "./HttpMethods.ts";

interface ILogin {
  userId: string;
  password: string;
}

interface ITodo {
  id?: string;
  userId: string;
  activitiesNo: string;
  subject: string;
  description?: string;
  status?: number;
  user?: IUser;
}

interface IUser {
  id?: string;
  userId: string;
  name: string;
  password: string;
  todos?: ITodo[];
}

export const login = (params: ILogin) => {
  return post({ path: "/auth/login", params });
};

/***********
 * Users   *
 ***********/

export const getUsers = () => {
  return get({ path: "/users" });
};

export const getUser = (id: string) => {
  return get({ path: `/users/${id}` });
};

export const createUser = (params: IUser) => {
  return post({ path: "/users", params });
};

export const updateUser = (id: string, params: IUser) => {
  return put({ path: `/users/${id}`, params });
};

export const deleteUser = (id: string) => {
  return del({ path: `/users/${id}` });
};

/**********
 * Todo   *
 **********/

export const getTodos = () => {
  return get({ path: "/todos" });
};

export const getTodo = (id: string) => {
  return get({ path: `/todos/${id}` });
};

export const getTodoByUserId = (userId: string) => {
  return get({ path: `/todos/user/${userId}` });
};

export const createTodo = (params: ITodo) => {
  return post({ path: "/todos", params });
};

export const createBatchTodo = (params: ITodo[]) => {
  return post({ path: "/todos/create/batch", params });
};

export const updateTodo = (id: string, params: ITodo) => {
  return put({ path: `/todos/${id}`, params });
};

export const updateStatusTodo = (id: string) => {
  return put({ path: `/todos/status/${id}` });
};

export const updateBatchStatusTodo = (params: {
  ids: string[];
  status: number | null;
}) => {
  return put({ path: "/todos/status/batch", params });
};

export const deleteTodo = (id: string) => {
  return del({ path: `/todos/${id}` });
};
