
const USE_MOCK = false; // om te kunnen runnen  zonder backend
const API_URL = "https://localhost:5001/api/Auth";

export async function loginApi(email, password) {
    if (USE_MOCK) {
        console.warn("Mock login gebruikt.");
        return { token: "mock-jwt-token", role: "Koper" };
    }

    const res = await fetch(`${API_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
    });

    if (!res.ok) throw new Error("Login mislukt.");
    return await res.json();
}

export async function registerApi(data) {
    if (USE_MOCK) {
        console.warn("Mock registratie gebruikt.");
        return { token: "mock-jwt-token" };
    }

    const res = await fetch(`${API_URL}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!res.ok) throw new Error("Registratie mislukt.");
    return await res.json();
}
