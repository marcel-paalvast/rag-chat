export default interface ChatStream {
  reader: ReadableStreamDefaultReader<Uint8Array>;
  continuationToken: string | null;
}