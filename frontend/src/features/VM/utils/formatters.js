// src/features/VM/utils/formatters.js

export function formatVeilingStatus(status) {
    if (status === null || status === undefined) return "-";

    const value = Number(status);
    if (Number.isNaN(value)) return String(status);

    switch (value) {
        case 0:
            return "Concept";
        case 1:
            return "Gepland";
        case 2:
            return "Lopend";
        case 3:
            return "Gepauzeerd";
        case 4:
            return "Beëindigd";
        default:
            return `Onbekend (${status})`;
    }
}

export function formatProductStatus(status) {
    if (status === null || status === undefined) return "-";

    const value = Number(status);
    if (Number.isNaN(value)) return String(status);

    switch (value) {
        case 0:
            return "In queue";
        case 1:
            return "Actief";
        case 2:
            return "Verkocht";
        case 3:
            return "Overgeslagen";
        default:
            return `Onbekend (${status})`;
    }
}

export function formatQueueStatus(status) {
    if (status === null || status === undefined) return "-";

    const value = Number(status);
    if (Number.isNaN(value)) return String(status);

    switch (value) {
        case 0:
            return "In queue";
        case 1:
            return "Actief";
        case 2:
            return "Verkocht";
        case 3:
            return "Overgeslagen";
        default:
            return `Onbekend (${status})`;
    }
}

export function formatTime(value) {
    if (!value) return "";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return String(value);

    return date.toLocaleTimeString("nl-NL", {
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
    });
}

export function formatDateTime(value) {
    if (!value) return "";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return String(value);

    return date.toLocaleString("nl-NL", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
    });
}
