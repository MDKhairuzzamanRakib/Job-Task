import axios from "axios";

export default axios.create({
  baseURL: "http://localhost:5045/api/users", // 👈 change port if different
});
