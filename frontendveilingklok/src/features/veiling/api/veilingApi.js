const API_BASE = "https://localhost:56418/api/veiling";
const PUBLIC_API_BASE = "https://localhost:56418/api/veiling-public";

export async function getActiveVeiling(token) {
    const res = await fetch(`${API_BASE}/active`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) return null;
    return res.json();
}

export async function startVeiling(token, date, time) {
    const body = time
        ? { veildatum: date, startTijd: time }
        : { veildatum: date };

    const res = await fetch(`${API_BASE}/start`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(body)
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error("Kon veiling niet starten: " + text);
    }

    return res.json();
}

export async function pauseVeiling(token, id) {
    await fetch(`${API_BASE}/${id}/pause`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function resumeVeiling(token, id) {
    await fetch(`${API_BASE}/${id}/resume`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function stopVeiling(token, id) {
    await fetch(`${API_BASE}/${id}/stop`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function placeBid(token, id) {
    const res = await fetch(`${API_BASE}/${id}/bod`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error("Bod plaatsen mislukt: " + text);
    }

    return res.json();
}

export async function fetchVeilingDagen(token) {
    const res = await fetch(`${PUBLIC_API_BASE}/dagen`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) {
        const text = await res.text();
        throw new Error("Kon veildagen niet laden: " + text);
    }

    return res.json();
}
