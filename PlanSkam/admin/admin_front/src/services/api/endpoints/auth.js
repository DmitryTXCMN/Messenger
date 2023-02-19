import axios from "../axios";

const endpoints = {
  registration: (data) => axios.post("/auth/register", data),
  login: (data) => axios.post("/auth/login", data)
};

export default endpoints;
