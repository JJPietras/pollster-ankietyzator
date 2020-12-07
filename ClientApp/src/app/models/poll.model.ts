interface Poll {
  pollId: number;
  authorId: number;
  tags: string;
  emails: string;
  nonAnonymous: boolean;
  archived: boolean;
  questions: Question [];
}
