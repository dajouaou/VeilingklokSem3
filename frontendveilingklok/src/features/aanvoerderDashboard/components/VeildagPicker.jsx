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
    "2025-12-26"
];

export default function VeildagPicker({ value, onChange, highlightedDates = [] }) {

    const isDayBlocked = (date) => {
        const iso = date.toISOString().split("T")[0];

        // Alleen zondag blokkeren (zaterdag mag nu wél)
        if (date.getDay() === 0) return false;

        // Feestdagen blokkeren blijft hetzelfde
        if (FEESTDAGEN.includes(iso)) return false;

        return true;
    };


    const highlight = highlightedDates
        .filter(d => d)
        .map(d => new Date(d));

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
            filterDate={isDayBlocked}
            highlightDates={highlight}
            popperPlacement="bottom-start"
            wrapperClassName="w-100"
        />
    );
}
