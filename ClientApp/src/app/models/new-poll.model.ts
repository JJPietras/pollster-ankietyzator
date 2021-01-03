interface NewPoll {
    authorId: number;
    title: string;
    description: string;
    tags: string;
    emails: string;
    nonAnonymous: boolean;
    archived: boolean;
    questions: NewQuestion[];
  }