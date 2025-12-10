import axios from "axios";

const http = axios.create({
    baseURL: "http://localhost:5143/api",
    headers: {
        "Content-Type": "application/json",
    },
});

export default http;
