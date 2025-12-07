// src/api/veilingmeester.js

import {
    getDashboard,
    startVeiling,
    nextProduct,
    closeCurrent,
} from "./VMApi";

export const VeilingmeesterApi = {
    dashboard: (veilingId) => getDashboard(veilingId),
    start: (veilingId) => startVeiling(veilingId),
    next: (veilingId) => nextProduct(veilingId),
    closeCurrent: (veilingId) => closeCurrent(veilingId),
};
