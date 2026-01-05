import React from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

// Feestdagen
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

function toSafeDate(d) {
    if (!d) return null;

    if (d instanceof Date) return d;

    // "YYYY-MM-DD" veilig parsen als lokale middernacht (voorkomt timezone shift)
    if (typeof d === "string" && /^\d{4}-\d{2}-\d{2}$/.test(d)) {
        return new Date(`${d}T00:00:00`);
    }

    const parsed = new Date(d);
    return Number.isNaN(parsed.getTime()) ? null : parsed;
}

export default function VeildagPicker({ value, onChange, highlightedDates = [] }) {
    const isDayAllowed = (date) => {
        const iso = date.toISOString().split("T")[0];

        // weekend blokkeren
        if (date.getDay() === 0 || date.getDay() === 6) return false;

        // feestdagen blokkeren
        if (FEESTDAGEN.includes(iso)) return false;

        return true;
    };

    const highlight = highlightedDates
        .filter((d) => d)
        .map((d) => toSafeDate(d))
        .filter((d) => d);

    return (
        <DatePicker
            selected={value}
            onChange={(date) => {
                if (!date) return;
                onChange(date);
            }}
            className="form-control"
            placeholderText="Kies een leverdatum"
            dateFormat="yyyy-MM-dd"
            filterDate={isDayAllowed}
            highlightDates={highlight}
            popperPlacement="bottom-start"
            wrapperClassName="w-100"
        />
    );
}
