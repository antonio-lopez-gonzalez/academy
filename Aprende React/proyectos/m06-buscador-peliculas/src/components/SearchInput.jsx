import { useRef, useEffect } from "react";

export default function SearchInput({ value, onChange }) {
  const inputRef = useRef(null);

  useEffect(() => {
    inputRef.current.focus();
  }, []);

  return (
    <input
      ref={inputRef}
      type="search"
      className="search-input"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder="Busca una película… (ej: matrix, star wars, inception)"
    />
  );
}
