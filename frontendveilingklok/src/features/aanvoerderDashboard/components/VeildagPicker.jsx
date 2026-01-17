import React from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const FEESTDAGEN = [
    "2025-01-01",
    "2025-04-18",
    "2025-04-20",
    "2025-04-21",
    "2025-05-29",
    "2025-06-08",
    "2025-06-09",
    "2025-12-25",
    "2025-12-26",
];

// Zet in Render (of lokaal) tijdelijk: VITE_ALLOW_SATURDAY=true
const ALLOW_SATURDAY = (import.meta.env.VITE_ALLOW_SATURDAY || "").toLowerCase() === "true";

export default function VeildagPicker({ value, onChange, highlightedDates = [] }) {
    const filterDate = (date) => {
        const iso = date.toISOString().split("T")[0];
        const day = date.getDay(); // 0=zo, 6=za

        // feestdagen nooit
        if (FEESTDAGEN.includes(iso)) return false;

        // zondag nooit
        if (day === 0) return false;

        // zaterdag alleen als toggle aan staat
        if (day === 6) return ALLOW_SATURDAY;

        // ma-vr wel
        return true;
    };

    const highlight = highlightedDates.filter(Boolean).map((d) => new Date(d));

    return (
        <DatePicker
            selected={value}
            onChange={(date) => date && onChange(date)}
            className="form-control"
            placeholderText="Kies een leverdatum"
            dateFormat="yyyy-MM-dd"
            filterDate={filterDate}
            highlightDates={highlight}
            popperPlacement="bottom-start"
            wrapperClassName="w-100"
        />
    );
}
