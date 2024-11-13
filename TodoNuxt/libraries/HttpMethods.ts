import axios, { AxiosResponse } from "axios";

const baseApiUrl = process.env.BASE_API_URL;

interface IParams {
  path: string;
  params?: any;
}

interface IApiResponse {
  data: any;
}

export const get = (params: IParams): Promise<AxiosResponse<IApiResponse>> => {
  return axios({
    baseURL: baseApiUrl,
    method: "GET",
    url: params.path,
    params: params.params,
  })
    .then((response) => {
      return response;
    })
    .catch((error) => {
      console.log("Failed to get response from server", error);
      throw new Error("Server Error", error);
    });
};

export const post = (params: IParams): Promise<AxiosResponse<IApiResponse>> => {
  return axios({
    baseURL: baseApiUrl,
    method: "POST",
    url: params.path,
    data: params.params,
  })
    .then((response) => {
      return response;
    })
    .catch((error) => {
      console.log("Failed to get response from server", error);
      throw new UnexpectedError("Server Error", error);
    });
};

export const put = (params: IParams): Promise<AxiosResponse<IApiResponse>> => {
  return axios({
    baseURL: baseApiUrl,
    method: "PUT",
    url: params.path,
    data: params.params,
  })
    .then((response) => {
      return response;
    })
    .catch((error) => {
      console.log("Failed to get response from server", error);
      throw new UnexpectedError("Server Error", error);
    });
};

export const del = (params: IParams): Promise<AxiosResponse<IApiResponse>> => {
  return axios({
    baseURL: baseApiUrl,
    method: "DELETE",
    url: params.path,
    data: params.params,
  })
    .then((response) => {
      return response;
    })
    .catch((error) => {
      console.log("Failed to get response from server", error);
      throw new UnexpectedError("Server Error", error);
    });
};
