import { useState, useEffect } from "react";

export default function VeildagDropdown({ token, value, onChange }) {
    const [dagen, setDagen] = useState([]);
    const [loading, setLoading] = useState(true);


    return (
        <select
            className="form-select"
            value={value}
            onChange={(e) => onChange(e.target.value)}
        >
            <option value="">-- Kies een veildatum --</option>
            {dagen.map((d) => (
                <option key={d} value={d}>
                    {new Date(d).toLocaleDateString("nl-NL")}
                </option>
            ))}
        </select>
    );
}
