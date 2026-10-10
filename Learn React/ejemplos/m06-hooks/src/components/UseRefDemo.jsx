import { useRef, useEffect } from "react";

export default function UseRefDemo() {
  const inputRef = useRef(null);
  const renderCountRef = useRef(0);
  renderCountRef.current++;

  useEffect(() => {
    inputRef.current.focus();
  }, []);

  return (
    <div>
      <input ref={inputRef} placeholder="Tengo foco automático" />
      <p>Renders: {renderCountRef.current}</p>
    </div>
  );
}
