import { API_BASE_URL } from "../../../config/apiBaseUrl";

export function iconUrl(path) {
    return `${API_BASE_URL}/icons/${path}`;
}
