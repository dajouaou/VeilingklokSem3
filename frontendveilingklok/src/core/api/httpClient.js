import axios from "axios";

export const httpClient = axios.create({
    baseURL: "http://localhost:56418/api",
    headers: {
        "Content-Type": "application/json",
    },
});

// Automatisch token meesturen
httpClient.interceptors.request.use((config) => {
    const token = localStorage.getItem("authToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});
