interface Poll2 {
    pollId: number;
    authorId: number;
    title: string;
    description: string;
    tags: string;
    emails: string;
    nonAnonymous: boolean;
    archived: boolean;
    questions: Question [];
  }