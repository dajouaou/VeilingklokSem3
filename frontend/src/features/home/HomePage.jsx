// src/pages/HomePage.jsx

import { Link } from "react-router-dom";

export default function HomePage() {
    return (
        <div style={{ padding: "24px" }}>
            <h1>Veilingklok</h1>
            <p>Kies een scherm:</p>

            <ul>
                <li>
                    <Link to="/veiling">Naar veiling (koper)</Link>
                </li>
                <li>
                    <Link to="/vm">Naar veilingmeester dashboard</Link>
                </li>
            </ul>
        </div>
    );
}
