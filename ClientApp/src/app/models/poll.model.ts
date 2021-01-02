interface Poll {
  pollId: number;
  authorId: number;
  authorEmail: string;
  authorName: string;
  title: string;
  tags: string;
  emails: string;
  nonAnonymous: boolean;
  archived: boolean;
  questions: Question [];
}
