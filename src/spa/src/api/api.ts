import ApiSettings from "./apiSettings";
import Assistant from "../models/assistant";
import ChatStream from "../models/chatStream";

export default class Api {
  private baseUri: string;
  private apiKey: string;

  constructor(config: ApiSettings) {
    this.baseUri = config.baseUri.replace(/\/$/, '');
    this.apiKey = config.apiKey;
  }

  async getAssistants(): Promise<Assistant[]> {
    const response = await fetch(`${this.baseUri}/assistants`, {
      method: 'GET',
      headers: {
        'x-functions-key': this.apiKey
      }
    });

    if (!response.ok) {
      throw new Error('Failed to fetch assistants');
    }

    return await response.json();
  }

  async getAssistantChat(
    assistantId: string,
    message: string,
    continuationToken?: string
  ): Promise<ChatStream> {
    const url = new URL(`${this.baseUri}/assistants/${assistantId}/chat`);
    url.searchParams.append('message', message);

    const headers: HeadersInit = {
      'x-functions-key': this.apiKey
    };

    if (continuationToken) {
      headers['Continuation-Token'] = continuationToken;
    }

    const response = await fetch(url.toString(), {
      method: 'GET',
      headers
    });

    if (!response.ok) {
      throw new Error('Failed to fetch assistant chat');
    }

    return {
      reader: response.body!.getReader(),
      continuationToken: response.headers.get('Continuation-Token')
    }
  }
}