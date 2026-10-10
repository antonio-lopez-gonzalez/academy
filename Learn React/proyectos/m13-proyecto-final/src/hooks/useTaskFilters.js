import { useState, useMemo } from "react";

export function useTaskFilters(tasks) {
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("all");
  const [priority, setPriority] = useState("all");
  const [label, setLabel] = useState("all");

  const filtered = useMemo(() => {
    const query = search.toLowerCase();

    return tasks.filter((task) => {
      const matchesSearch =
        task.title.toLowerCase().includes(query) ||
        task.description?.toLowerCase().includes(query);

      const matchesStatus = status === "all" || task.status === status;
      const matchesPriority = priority === "all" || task.priority === priority;
      const matchesLabel = label === "all" || task.labels?.includes(label);

      return matchesSearch && matchesStatus && matchesPriority && matchesLabel;
    });
  }, [tasks, search, status, priority, label]);

  const availableLabels = useMemo(() => {
    const labels = new Set();
    tasks.forEach((t) => t.labels?.forEach((l) => labels.add(l)));
    return Array.from(labels);
  }, [tasks]);

  return {
    filters: { search, status, priority, label },
    setSearch,
    setStatus,
    setPriority,
    setLabel,
    filtered,
    availableLabels,
  };
}
