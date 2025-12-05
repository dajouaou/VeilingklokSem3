const API = "https://localhost:56418/api/veiling";


export async function getActiveVeiling(token) {
    const res = await fetch(`${API}/1`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return res.ok ? res.json() : null;
}

export async function startVeiling(token, date) {
    const res = await fetch(`${API}/start`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ veildatum: date })
    });
    return res.json();
}

export async function pauseVeiling(token, id) {
    return fetch(`${API}/${id}/pause`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function resumeVeiling(token, id) {
    return fetch(`${API}/${id}/resume`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function stopVeiling(token, id) {
    return fetch(`${API}/${id}/stop`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` }
    });
}

export async function placeBid(token, veilingId, veilingProductId) {
    const res = await fetch(`${API}/${veilingId}/bid`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ veilingProductId })
    });

    return res.json();
}

export async function getCurrentLot(veilingId, token) {
    const res = await fetch(`${API}/${veilingId}/current`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return res.json();
}

export async function getQueue(veilingId, token) {
    const res = await fetch(`${API}/${veilingId}/queue`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    return res.json();
}
