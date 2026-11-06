import fs from "node:fs/promises";
import path from "node:path";

/**
 * Catálogo de documentos en JSON.
 *
 * S3 no permite buscar por metadatos de negocio con la potencia de una base de
 * datos. En una aplicación real este catálogo sería PostgreSQL; aquí es un
 * fichero para que el proyecto no requiera servicios extra.
 */
export class Catalog {
  constructor(file) {
    this.file = file;
    this.chain = Promise.resolve();
  }

  async #read() {
    try {
      return JSON.parse(await fs.readFile(this.file, "utf8"));
    } catch (error) {
      if (error.code === "ENOENT") return {};
      throw error;
    }
  }

  async #write(data) {
    await fs.mkdir(path.dirname(this.file), { recursive: true });
    const tmp = `${this.file}.${process.pid}.tmp`;
    await fs.writeFile(tmp, JSON.stringify(data, null, 2));
    await fs.rename(tmp, this.file);
  }

  #mutate(mutator) {
    const run = this.chain.then(async () => {
      const data = await this.#read();
      const result = mutator(data);
      await this.#write(data);
      return result;
    });
    this.chain = run.catch(() => {});
    return run;
  }

  create(doc) {
    return this.#mutate((data) => {
      data[doc.id] = doc;
      return doc;
    });
  }

  update(id, patch) {
    return this.#mutate((data) => {
      if (!data[id]) return null;
      data[id] = { ...data[id], ...patch, updatedAt: new Date().toISOString() };
      return data[id];
    });
  }

  async get(id) {
    return (await this.#read())[id] ?? null;
  }

  async list({ tenant } = {}) {
    const docs = Object.values(await this.#read());
    const filtered = tenant ? docs.filter((doc) => doc.tenant === tenant) : docs;
    return filtered.sort((a, b) => b.createdAt.localeCompare(a.createdAt));
  }
}
